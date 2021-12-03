import cv2
import mediapipe as mp
import time
import numpy as np
import socket
import select

TIMEOUT = 3


def parse_landmark_values(landmark):
    landmark_values = str(landmark).strip("\n").split("\n")
    xyz_vis = np.zeros(4, dtype=float)
    for i, variables in enumerate(landmark_values):
        var_split = variables.split(": ")
        xyz_vis[i] = float(var_split[1])
    return xyz_vis


def connect_socket():
    host, port = "127.0.0.1", 25001
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect((host, port))
    print("Connected")
    return sock


def send_over_socket(sock: socket, unity_message: str):
    try:
        sock.sendall(unity_message.encode("UTF-8"))
    except:
        sock.close()
        sock = connect_socket()
        sock.sendall(unity_message.encode("UTF-8"))


def recv_over_socket(sock):
    ready = select.select([sock], [], [], TIMEOUT)
    if ready[0]:
        my_received_data = sock.recv(1024).decode("UTF-8")
        print(my_received_data)
        return True
    else:
        print("Socket Closed")
        sock.close()
        return False


def main():
    """
    Format of entire array (unity_message) is [ (object1) | (object2) | .... ]
    """
       
    # Pose Detection Setup
    mp_pose = mp.solutions.pose
    pose = mp_pose.Pose()
    mp_draw = mp.solutions.drawing_utils
    cap = cv2.VideoCapture(0)  # 0 = Webcam
    #cap = cv2.VideoCapture("videos/a.mp4")  # 0 = Webcam
    p_time = 0

    # Socket Setup
    sock = connect_socket()

    # Loop over Input Video
    while True:
        sock = connect_socket()
        success, img = cap.read()
        img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        results = pose.process(img_rgb)
        unity_message = "["
        landmark_count = 0
        if results.pose_landmarks:
            mp_draw.draw_landmarks(img, results.pose_landmarks, mp_pose.POSE_CONNECTIONS)
            for iid, lm in enumerate(results.pose_landmarks.landmark):
                parsed_values = parse_landmark_values(lm)
                unity_message += "({:.3f}, {:.3f}, {:.3f}, {:.3f})".format(*parsed_values)  # xyz and visibility
                unity_message = unity_message + "|" if (len(results.pose_landmarks.landmark)-1 > iid) else unity_message
                landmark_count = landmark_count + 1

                h, w, c = img.shape  # height, width and c?
                cx, cy = int(lm.x*w), int(lm.y*h)
                cv2.circle(img, (cx, cy), 5, (255, 0, 0), cv2.FILLED)
            unity_message += ']'
            #print(unity_message)
            send_over_socket(sock, unity_message)
            sock.close()

        c_time = time.time()
        fps = 1/(c_time-p_time)
        p_time = c_time

        #print("receivedData made next change")
        cv2.putText(img, str(int(fps)), (50, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 0, 0), 3)
        cv2.imshow("Image", img)
        cv2.waitKey(1)


if __name__ == "__main__":
    main()

