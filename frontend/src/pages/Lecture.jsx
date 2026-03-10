import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { apiRequest } from "../api/api";
import Flashcards from "../components/Flashcards";
import Quiz from "../components/Quiz";

export default function Lecture() {

    const { id } = useParams();

    const [lecture, setLecture] = useState(null);
    const [summary, setSummary] = useState("");

    async function loadLecture() {
        const lectures = await apiRequest("/lectures/mine");
        const current = lectures.find(l => l.id == id);
        setLecture(current);
    }

    useEffect(() => {
        loadLecture();
        loadSummary();
    }, []);

    async function extractText() {
        await apiRequest(`/lectures/${id}/extract-text`, "POST");
        loadLecture();
    }

    async function generateSummary() {
        await apiRequest(`/lectures/${id}/generate-summary`, "POST");
        const data = await apiRequest(`/lectures/${id}/summary`);
        setSummary(data.summary);
        loadLecture();
    }

    async function generateFlashcards() {
        await apiRequest(`/lectures/${id}/generate-flashcards`, "POST");
        loadLecture();
    }

    async function generateQuiz() {
        await apiRequest(`/lectures/${id}/generate-quiz`, "POST");
        loadLecture();
    }

    async function loadSummary() {
        try {
            const data = await apiRequest(`/lectures/${id}/summary`);
            setSummary(data.summary);
        } catch {
            // summary not generated yet
        }
    }

    if (!lecture) return <div>Loading...</div>;

    return (
        <div>

            <h1>{lecture.title}</h1>

            <p>Status: {lecture.status}</p>

            {lecture.status === "Uploaded" && (
                <button onClick={extractText}>
                    Extract Text
                </button>
            )}

            {lecture.status === "TextExtracted" && (
                <button onClick={generateSummary}>
                    Generate Summary
                </button>
            )}

            {lecture.status === "SummaryGenerated" && (
                <button onClick={generateFlashcards}>
                    Generate Flashcards
                </button>
            )}

            {lecture.status === "FlashcardsGenerated" && (
                <button onClick={generateQuiz}>
                    Generate Quiz
                </button>
            )}

            <h2>Summary</h2>

            <p>{summary}</p>

            <Flashcards lectureId={id} />

            <Quiz lectureId={id} />
        </div>
    );
}