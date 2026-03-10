import { useEffect, useState } from "react";
import { apiRequest } from "../api/api";
import UploadLecture from "../components/UploadLecture";
import { Link } from "react-router-dom";

export default function Dashboard() {
    const [lectures, setLectures] = useState([]);

    async function loadLectures() {
        try {
            const data = await apiRequest("/lectures/mine");
            setLectures(data);
        } catch (err) {
            alert("Failed to load lectures");
        }
    }

    useEffect(() => {
        loadLectures();
    }, []);

    return (
        <div>
            <h1>StudyMate Dashboard</h1>

            <UploadLecture onUpload={loadLectures} />

            <h2>My Lectures</h2>
            
            <ul>
                {lectures.map((lecture) => (
                    <li key={lecture.id}>
                        <Link to={`/lecture/${lecture.id}`}>
                            {lecture.title}
                        </Link>
                        {" "}— {lecture.status}
                    </li>
                ))}
            </ul>

        </div>
    );
}