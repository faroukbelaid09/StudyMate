const API_BASE = "https://localhost:7204";

export async function apiRequest(endpoint, method = "GET", body) {
  const token = localStorage.getItem("token");

  const res = await fetch(`${API_BASE}${endpoint}`, {
    method,
    headers: {
      "Content-Type": "application/json",
      Authorization: token ? `Bearer ${token}` : ""
    },
    body: body ? JSON.stringify(body) : undefined
  });

  if (!res.ok) {
    throw new Error(await res.text());
  }

  const text = await res.text();

  try {
    return JSON.parse(text);
  } catch {
    return text;
  }
}