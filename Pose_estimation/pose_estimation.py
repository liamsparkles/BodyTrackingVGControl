import cv2
import mediapipe as mp
import time
import numpy as np
import socket
import time

host, port = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))

def parselandmarkvalues(landmark):
    landmark_vals = str(landmark).strip("\n").split("\n")
    xyz_vis = np.zeros(4, dtype=float)
    for i, variables in enumerate(landmark_vals):
        var_split = variables.split(": ")
        xyz_vis[i] = float(var_split[1])
    return xyz_vis

mpPose = mp.solutions.pose
pose = mpPose.Pose()
mpDraw = mp.solutions.drawing_utils

cap = cv2.VideoCapture(0)
#cap = cv2.VideoCapture('videos/a.mp4')
pTime = 0

with open('testfile.csv', 'w') as f:
    iter_num = 0
    while True:
        success, img = cap.read()
        imgRGB = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        results = pose.process(imgRGB)
        #print(results.pose_landmarks)
        if results.pose_landmarks:
            for i, landmark in enumerate(results.pose_landmarks.landmark):
                parsed_vals = parselandmarkvalues(landmark)
                a = [i+1]  # position number (1 - 33)
                a.extend(parsed_vals)
                sendData(a)
                f.write(str(iter_num) + ","
                        + str(parsed_vals[0]) + ","
                        + str(parsed_vals[1]) + ","
                        + str(parsed_vals[2]) + ","
                        + str(parsed_vals[3]) + "\n")
            iter_num += 1
            mpDraw.draw_landmarks(img, results.pose_landmarks, mpPose.POSE_CONNECTIONS)
            for iid, lm in enumerate(results.pose_landmarks.landmark):
                h, w, c = img.shape  # height, width and c?
                #print(iid, lm)
                cx, cy = int(lm.x*w), int(lm.y*h)
                cv2.circle(img, (cx, cy), 5, (255, 0, 0), cv2.FILLED)

        cTime = time.time()
        fps = 1/(cTime-pTime)
        pTime = cTime
        cv2.putText(img, str(int(fps)), (50, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 0, 0), 3)
        cv2.imshow("Image", img)
        cv2.waitKey(1)

def sendData(position):
    posString = ','.join(map(str, position)) #Converting Vector5 to a string
    # values are (positionIndex, x, y, z, visibility)
    print(posString)
    sock.sendall(posString.encode("UTF-8")) #Converting string to Byte, and sending it to C#

