import { useEffect, useState } from "react";
import { apiRequest } from "../api/api";

export default function Flashcards({ lectureId }) {

  const [flashcards, setFlashcards] = useState([]);
  const [index, setIndex] = useState(0);
  const [showAnswer, setShowAnswer] = useState(false);

  async function loadFlashcards() {
    try {
      const data = await apiRequest(`/lectures/${lectureId}/flashcards`);
      setFlashcards(data);
    } catch {
      console.log("No flashcards yet");
    }
  }

  useEffect(() => {
    loadFlashcards();
  }, []);

  if (flashcards.length === 0) {
    return <p>No flashcards generated.</p>;
  }

  const card = flashcards[index];

  function nextCard() {
    setIndex((index + 1) % flashcards.length);
    setShowAnswer(false);
  }

  function prevCard() {
    setIndex((index - 1 + flashcards.length) % flashcards.length);
    setShowAnswer(false);
  }

  return (
    <div>

      <h2>Flashcards</h2>

      <div
        style={{
          border: "1px solid black",
          padding: "20px",
          width: "400px",
          margin: "20px auto",
          textAlign: "center",
          cursor: "pointer"
        }}
        onClick={() => setShowAnswer(!showAnswer)}
      >

        {showAnswer ? card.answer : card.question}

      </div>

      <button onClick={prevCard}>Previous</button>
      <button onClick={nextCard}>Next</button>

    </div>
  );
}