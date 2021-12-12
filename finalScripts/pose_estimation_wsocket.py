import cv2
import mediapipe as mp
import time
import numpy as np
import socket


def parse_landmark_values(landmark):
    """
    Parses the landmark values from the mediapipe solution for each object
    """
    landmark_values = str(landmark).strip("\n").split("\n")
    xyz_vis = np.zeros(4, dtype=float)
    for i, variables in enumerate(landmark_values):
        var_split = variables.split(": ")
        xyz_vis[i] = float(var_split[1])
    return xyz_vis


def main():
    """
    Format of entire array (unity_message) is [ (object1) | (object2) | .... ]
    Code taken from open source
    Syed Abdul Gaffar Shakhadri (Pose Detection Open Source)
    https://www.analyticsvidhya.com/blog/2021/05/pose-estimation-using-opencv/
    Communication between Python and Unity Open Source
    https://github.com/CanYouCatchMe01/CSharp-and-Python-continuous-communication
    For both communication and open source, these were algamated together and 
    changed slightly
    """

    # Pose Detection Setup
    mp_pose = mp.solutions.pose
    pose = mp_pose.Pose()
    mp_draw = mp.solutions.drawing_utils
    cap = cv2.VideoCapture(0)  # 0 = Webcam
    p_time = 0

    # Socket Setup
    host, port = "127.0.0.1", 25001
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect((host, port))

    # Loop over Input Video
    while True:
        success, img = cap.read()
        img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        results = pose.process(img_rgb)
        unity_message = "["
        landmark_count = 0
        if results.pose_landmarks:
            # If there are landmarks, parse and send all of them
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
            print(unity_message)
            # Send all data over to unity
            sock.sendall(unity_message.encode("UTF-8"))

        # Timing variables for displaying frames per second to user
        c_time = time.time()
        fps = 1/(c_time-p_time)
        p_time = c_time

        cv2.putText(img, str(int(fps)), (50, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 0, 0), 3)
        cv2.imshow("Image", img)
        cv2.waitKey(1)


if __name__ == "__main__":
    main()

