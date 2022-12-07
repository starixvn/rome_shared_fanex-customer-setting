import { useRouter } from "next/router";
import React from "react";

const Unauthorized = () => {
  const router = useRouter();

  async function handleLogin() {
    const resp = await fetch("/api/auth/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        userId: 12345,
        username: "alvin",
        roleId: 4,
        subAccId: 0,
      }),
    });
    console.log(await resp.json());
    router.push("/");
  }

  return (
    <section>
      <h1>Unauthorized</h1>
      <br />
      <p>You do not have access to the requested page.</p>
      <button onClick={handleLogin}>Login</button>
    </section>
  );
};

export default Unauthorized;
