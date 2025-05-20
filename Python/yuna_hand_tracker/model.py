from pydantic import BaseModel
from typing import List


class Point3D(BaseModel):
    x: float
    y: float
    z: float


class LandmarkList(BaseModel):
    keypoints: List[Point3D]
