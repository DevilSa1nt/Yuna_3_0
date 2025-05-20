from fastapi import FastAPI, UploadFile, File
from fastapi.responses import JSONResponse
import numpy as np
import cv2

from model import Point3D
from hand_tracker import HandTracker

app = FastAPI()
tracker = HandTracker()

@app.post("/track")
async def track(file: UploadFile = File(...)):
    try:
        image_bytes = await file.read()
        nparr = np.frombuffer(image_bytes, np.uint8)
        image = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

        hands = tracker.extract_all_hands(image)

        return {"hands": hands}
    except Exception as e:
        return JSONResponse(status_code=500, content={"error": str(e)})

