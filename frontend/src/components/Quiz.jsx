import { useEffect, useState } from "react";
import { apiRequest } from "../api/api";

export default function Quiz({ lectureId }) {

  const [questions, setQuestions] = useState([]);
  const [answers, setAnswers] = useState({});
  const [result, setResult] = useState(null);

  async function loadQuiz() {
    try {
      const data = await apiRequest(`/lectures/${lectureId}/quiz`);
      setQuestions(data);
    } catch {
      console.log("Quiz not generated yet");
    }
  }

  useEffect(() => {
    loadQuiz();
  }, []);

  function selectAnswer(questionId, option) {
    setAnswers({
      ...answers,
      [questionId]: option
    });
  }

  async function submitQuiz() {
    const data = await apiRequest(
      `/lectures/${lectureId}/quiz/submit`,
      "POST",
      { answers }
    );

    setResult(data);
  }

  if (questions.length === 0) {
    return <p>No quiz generated yet.</p>;
  }

  return (
    <div>

      <h2>Quiz</h2>

      {questions.map((q) => (
        <div key={q.id} style={{ marginBottom: "20px" }}>

          <p><b>{q.question}</b></p>

          {["A", "B", "C", "D"].map((option) => (
            <div key={option}>

              <label>

                <input
                  type="radio"
                  name={q.id}
                  value={option}
                  onChange={() => selectAnswer(q.id, option)}
                />

                {option}: {q[`option${option}`]}

              </label>

            </div>
          ))}

        </div>
      ))}

      <button onClick={submitQuiz}>
        Submit Quiz
      </button>

      {result && (
        <h3>
          Score: {result.score} / {result.total}
        </h3>
      )}

    </div>
  );
}