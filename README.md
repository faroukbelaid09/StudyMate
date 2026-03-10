# StudyMate API Documentation

## Base URL

```
http://localhost:5000
```

All protected endpoints require **JWT authentication**.

Add the token to the header:

```
Authorization: Bearer <token>
```

---

# Authentication

## Register

Create a new user account.

**POST** `/auth/register`

### Request

```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

### Response

```json
{
  "token": "JWT_TOKEN"
}
```

---

## Login

Authenticate an existing user.

**POST** `/auth/login`

### Request

```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

### Response

```json
{
  "token": "JWT_TOKEN"
}
```

---

# Lectures

## Upload Lecture

Upload a lecture PDF file.

**POST** `/lectures/upload`

### Request (Form Data)

| Field | Type   | Description      |
| ----- | ------ | ---------------- |
| title | string | Lecture title    |
| file  | pdf    | Lecture PDF file |

### Response

```
Lecture uploaded.
```

---

## Get My Lectures

Returns all lectures belonging to the authenticated user.

**GET** `/lectures/mine`

### Response

```json
[
  {
    "id": 1,
    "title": "Microbiology",
    "uploadedAt": "2026-03-06T10:00:00",
    "status": "Uploaded"
  }
]
```

---

## Download Lecture

Download the uploaded lecture PDF.

**GET** `/lectures/{id}/download`

### Response

Returns the PDF file.

---

## Delete Lecture

Delete a lecture and its associated data.

**DELETE** `/lectures/{id}`

### Response

```
Lecture deleted.
```

---

# Processing

## Extract Text From PDF

Extracts text from the uploaded lecture PDF.

**POST** `/lectures/{id}/extract-text`

### Response

```
Text extracted.
```

---

# AI Summary

## Generate Summary

Generates a summary for the lecture.

**POST** `/lectures/{id}/generate-summary`

### Response

```
Summary generated.
```

---

## Get Summary

Retrieve the generated lecture summary.

**GET** `/lectures/{id}/summary`

### Response

```json
{
  "lectureId": 1,
  "summary": "Lecture summary text..."
}
```

---

# Flashcards

## Generate Flashcards

Generate study flashcards from the lecture text.

**POST** `/lectures/{id}/generate-flashcards`

### Response

```
Flashcards generated.
```

---

## Get Flashcards

Retrieve generated flashcards.

**GET** `/lectures/{id}/flashcards`

### Response

```json
[
  {
    "id": 1,
    "question": "What is PCR?",
    "answer": "Polymerase Chain Reaction"
  }
]
```

---

# Quiz

## Generate Quiz

Generate quiz questions based on the lecture content.

**POST** `/lectures/{id}/generate-quiz`

### Response

```
Quiz generated.
```

---

## Get Quiz

Retrieve quiz questions for the lecture.

**GET** `/lectures/{id}/quiz`

### Response

```json
[
  {
    "id": 1,
    "question": "What does PCR stand for?",
    "optionA": "Protein Chain Reaction",
    "optionB": "Polymerase Chain Reaction",
    "optionC": "Primary Cell Reaction",
    "optionD": "Protein Cell Regulation",
    "correctAnswer": "B"
  }
]
```

---

## Submit Quiz

Submit quiz answers and receive a score.

**POST** `/lectures/{id}/quiz/submit`

### Request

```json
{
  "answers": {
    "1": "B",
    "2": "A"
  }
}
```

### Response

```json
{
  "score": 1,
  "total": 2
}
```

---

# Lecture Processing Status

Each lecture has a **processing status** representing its current stage.

Possible statuses:

```
Uploaded
TextExtracted
SummaryGenerated
FlashcardsGenerated
QuizGenerated
Failed
```

Example:

```json
{
  "id": 1,
  "title": "Microbiology",
  "status": "SummaryGenerated"
}
```

---

# Project Architecture

Backend technologies used:

```
ASP.NET Core Web API
Entity Framework Core
SQL Server
JWT Authentication
PDF Text Extraction
AI Processing Pipeline
```