import cv2
import mediapipe as mp
import numpy as np
from typing import List
from model import Point3D


class HandTracker:
    def __init__(self):
        self.hands = mp.solutions.hands.Hands(
            static_image_mode=False,
            max_num_hands=2,
            min_detection_confidence=0.7,
            min_tracking_confidence=0.5
        )

    def extract_keypoints(self, image: np.ndarray) -> List[Point3D]:
        image_rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        results = self.hands.process(image_rgb)

        keypoints = []
        if results.multi_hand_landmarks:
            for hand_landmarks in results.multi_hand_landmarks:
                for lm in hand_landmarks.landmark:
                    keypoints.append(Point3D(x=lm.x, y=lm.y, z=lm.z))
                break  # только первая рука

        return keypoints

    def extract_all_hands(self, image: np.ndarray) -> List[dict]:
        image_rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        results = self.hands.process(image_rgb)

        output = []

        if results.multi_hand_landmarks and results.multi_handedness:
            for hand_landmarks, handedness in zip(results.multi_hand_landmarks, results.multi_handedness):
                label = handedness.classification[0].label  # 'Left' or 'Right'
                keypoints = [
                    Point3D(x=lm.x, y=lm.y, z=lm.z)
                    for lm in hand_landmarks.landmark
                ]
                output.append({
                    "label": label,
                    "keypoints": [kp.dict() for kp in keypoints]
                })

        return output


