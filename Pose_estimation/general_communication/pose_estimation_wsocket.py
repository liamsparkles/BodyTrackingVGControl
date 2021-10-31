import cv2
import mediapipe as mp
import time
import numpy as np
import socket


def parselandmarkvalues(landmark):
    landmark_vals = str(landmark).strip("\n").split("\n")
    xyz_vis = np.zeros(4, dtype=float)
    for i, variables in enumerate(landmark_vals):
        var_split = variables.split(": ")
        xyz_vis[i] = float(var_split[1])
    return xyz_vis


def main():
    """
    Format of entire array (msg_unity) is [ (object1) | (object2) | .... ]
    """
       
    # Pose Detection Setup
    mpPose = mp.solutions.pose
    pose = mpPose.Pose()
    mpDraw = mp.solutions.drawing_utils
    cap = cv2.VideoCapture(0) # 0 = Webcam
    pTime = 0

    # Socket Setup
    host, port = "127.0.0.1", 25002
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect((host, port))

    # Loop over Input Video
    while True:
        success, img = cap.read()
        imgRGB = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        results = pose.process(imgRGB)
        msg_unity = "["
        landmark_count = 0
        if results.pose_landmarks:
            mpDraw.draw_landmarks(img, results.pose_landmarks, mpPose.POSE_CONNECTIONS)
            for iid, lm in enumerate(results.pose_landmarks.landmark):
                parsed_vals = parselandmarkvalues(lm)
                msg_unity += "({:.3f}, {:.3f}, {:.3f}, {:.3f})".format(*parsed_vals))  # xyz and visibility
                msg_unity = msg_unity+"|" if (len(results.pose_landmarks.landmark)-1 > iid) else msg_unity
                landmark_count = landmark_count + 1

                h, w, c = img.shape  # height, width and c?
                cx, cy = int(lm.x*w), int(lm.y*h)
                cv2.circle(img, (cx, cy), 5, (255, 0, 0), cv2.FILLED)
            msg_unity += ']'
            print(msg_unity)
            sock.sendall(msg_unity.encode("UTF-8"))

        cTime = time.time()
        fps = 1/(cTime-pTime)
        pTime = cTime
        cv2.putText(img, str(int(fps)), (50, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 0, 0), 3)
        cv2.imshow("Image", img)
           print("receivedData made next change")
        cv2.waitKey(1)
    return 0


if "__name__" == __main__:
    main()

