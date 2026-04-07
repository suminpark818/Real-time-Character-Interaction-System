# Real-Time Emotion-Driven VTuber Interaction System

A real-time AI interaction system that analyzes a user's facial expressions and voice emotions, allowing a VTuber avatar to respond dynamically.
The system integrates computer vision, audio emotion recognition, and Live2D avatar control in Unity to create immersive interactive experiences.

---

# Overview

This project implements a real-time VTuber interaction system using AI-based emotion recognition.
The system captures webcam video and microphone audio, analyzes emotional states, and reflects the results in a Live2D avatar in Unity.

By combining multimodal emotion analysis with character animation, the avatar can respond naturally to the user's emotional state.

---

# Key Features

* Real-time facial emotion recognition
* Audio-based emotion analysis
* Live2D avatar control in Unity
* Automatic Cubism parameter detection and connection
* Real-time facial tracking (eyes, mouth, head movement)
* Multimodal emotion-driven character interaction

---

# System Architecture

User Input
(Webcam + Microphone)

↓

Emotion Analysis
(Face Emotion Model + Audio Emotion Model)

↓

Emotion Data Processing
(Python Flask Server)

↓

Avatar Control
(Unity + Live2D Cubism)

↓

VTuber Interaction Output

---

# Tech Stack

### AI / Machine Learning

* Python
* PyTorch
* ResNet18 (Emotion Recognition)
* YOLOv5-face
* OpenCV

### Real-Time System

* Unity
* Live2D Cubism SDK
* C#

### Backend

* Flask (Emotion Analysis Server)

---

# How to Run

### 1. Start the Emotion Server

First, run the Flask emotion analysis server.

```bash
cd model
python flask_emotion_server.py
```

### 2. Run the Unity Project

After the server is running, open the Unity project and start the scene.
The Unity application will send webcam and audio data to the Flask server and receive emotion analysis results in real time.

---

# How It Works

1. Webcam and microphone capture the user's facial expressions and voice.
2. The data is sent to the emotion analysis server.
3. The AI model predicts emotional states.
4. The result is returned to Unity.
5. The Live2D avatar updates facial parameters and expressions in real time.

---

# Demo




---

# Author

Sumin Park

AI Interaction / Real-Time Systems / VTuber Technology

---

# 한국어 설명

## 프로젝트 개요

이 프로젝트는 **사용자의 얼굴 표정과 음성을 분석하여 VTuber 캐릭터가 실시간으로 반응하는 인터랙션 시스템**입니다.
웹캠 영상과 마이크 음성을 입력받아 감정을 분석하고, 그 결과를 Unity의 Live2D 아바타에 반영하여 캐릭터가 감정에 맞는 표정을 표현하도록 구현했습니다.

얼굴 감정 분석과 음성 감정 분석을 함께 사용하는 **멀티모달 감정 인식 시스템**을 기반으로 합니다.

---

## 실행 방법

### 1. 감정 분석 서버 실행

먼저 감정 분석을 담당하는 Flask 서버를 실행해야 합니다.

```bash
cd model
python flask_emotion_server.py
```

---

### 2. Unity 실행

Flask 서버가 실행된 후 Unity 프로젝트를 실행합니다.

Unity는 웹캠 영상과 오디오 데이터를 Flask 서버로 전송하고, 서버에서 분석된 감정 결과를 받아 Live2D 캐릭터의 표정과 움직임에 실시간으로 반영합니다.

---

## 주요 기능

* 실시간 얼굴 감정 인식
* 음성 기반 감정 분석
* Unity Live2D 캐릭터 실시간 제어
* Cubism 파라미터 자동 연결
* 얼굴 추적 기반 눈 / 입 / 고개 움직임 반영
* 멀티모달 감정 기반 캐릭터 인터랙션 시스템


