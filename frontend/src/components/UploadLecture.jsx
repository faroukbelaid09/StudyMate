import { useState } from "react";

export default function UploadLecture({ onUpload }) {
  const [title, setTitle] = useState("");
  const [file, setFile] = useState(null);

  async function handleUpload(e) {
  e.preventDefault();

  if (!file) {
    alert("Select a file");
    return;
  }

  const formData = new FormData();
  formData.append("Title", title);
  formData.append("File", file);

  const token = localStorage.getItem("token");

  const res = await fetch("https://localhost:7204/lectures/upload", {
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`
    },
    body: formData
  });

  if (!res.ok) {
    const text = await res.text();
    console.log(text);
    alert("Upload failed");
    return;
  }

  onUpload();
}

  return (
    <form onSubmit={handleUpload}>
      <h2>Upload Lecture</h2>

      <input
        placeholder="Lecture Title"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
      />

      <input
        type="file"
        accept="application/pdf"
        onChange={(e) => setFile(e.target.files[0])}
      />

      <button type="submit">Upload</button>
    </form>
  );
}