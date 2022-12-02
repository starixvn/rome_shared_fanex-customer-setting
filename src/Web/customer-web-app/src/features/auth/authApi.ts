export async function sso(): Promise<string> {
  const response = await fetch("/api/auth/login", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
  });
  const result = await response.text();

  return result;
}
