import cv2
import mediapipe as mp
import socket
import json

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
unity_ip = "127.0.0.1"
unity_port = 5053

mp_hands = mp.solutions.hands
hands = mp_hands.Hands(max_num_hands=2, min_detection_confidence=0.7)
cap = cv2.VideoCapture(0)

def is_fist(landmarks):
    tips_ids = [4, 8, 12, 16, 20]
    folded = 0
    for tip_id in tips_ids:
        if landmarks[tip_id].y > landmarks[tip_id - 2].y:
            folded += 1
    return folded >= 4

while cap.isOpened():
    success, image = cap.read()
    if not success:
        continue

    image = cv2.flip(image, 1)
    image_rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    results = hands.process(image_rgb)

    send_data = {}

    if results.multi_hand_landmarks and results.multi_handedness:
        for idx, hand_info in enumerate(results.multi_handedness):
            label = hand_info.classification[0].label
            landmarks = results.multi_hand_landmarks[idx]
            wrist = landmarks.landmark[mp_hands.HandLandmark.WRIST]

            if label == "Right":
                send_data["Right"] = {"x": wrist.x, "y": wrist.y}

            elif label == "Left":
                # ✨ 判断是否握拳
                if is_fist(landmarks.landmark):
                    send_data["LeftGesture"] = "fist"
                else:
                    send_data["LeftGesture"] = "open"

                # ✅ 添加食指指尖坐标
                index_tip = landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP]
                send_data["LeftFinger"] = {"x": index_tip.x, "y": index_tip.y}

    if send_data:
        sock.sendto(json.dumps(send_data).encode(), (unity_ip, unity_port))

    cv2.imshow("MediaPipe", image)
    if cv2.waitKey(1) & 0xFF == 27:
        break

cap.release()

