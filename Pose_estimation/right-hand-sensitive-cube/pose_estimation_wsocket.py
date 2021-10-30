import cv2
import mediapipe as mp
import time
import numpy as np
import socket #abs990

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

#socket setup - abs990
host, port = "127.0.0.1", 25002
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))

while True:
    success, img = cap.read()
    imgRGB = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    results = pose.process(imgRGB)
    msg_unity = ""
    landmark_count = 0
    if results.pose_landmarks:
        for landmark in results.pose_landmarks.landmark:
            parsed_vals = parselandmarkvalues(landmark)
            #construct message to unity. Send right hand coordinates
            if landmark_count in [16,18]:
                msg_unity = msg_unity+str("{:.3f}".format(parsed_vals[0]))+","+str("{:.3f}".format(parsed_vals[1]))+","+str("{:.3f}".format(parsed_vals[2]))+"|"
            elif landmark_count == 20:
                msg_unity = msg_unity+str("{:.3f}".format(parsed_vals[0]))+","+str("{:.3f}".format(parsed_vals[1]))+","+str("{:.3f}".format(parsed_vals[2]))
            landmark_count = landmark_count + 1
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
    if msg_unity == "":
        msg_unity = "0,0,0"
    sock.sendall(msg_unity.encode("UTF-8"))
    print("receivedData made next change")
    cv2.waitKey(1)
