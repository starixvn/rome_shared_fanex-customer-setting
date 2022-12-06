import React from "react";
import { useLoginMutation } from "./authApi";
import { useRouter } from "next/router";
import { useCookies } from "react-cookie";

const Unauthorized = () => {
  const router = useRouter();
  const [login] = useLoginMutation();
  const [cookies, setCookie] = useCookies(["accessToken"]);
  return (
    <section>
      <h1>Unauthorized</h1>
      <br />
      <p>You do not have access to the requested page.</p>
      <button
        onClick={async () => {
          var response = await login({
            userId: 12345,
            username: "alvin",
            roleId: 4,
            subAccId: 0,
          }).unwrap();

          setCookie("accessToken", response.accessToken, { path: "/" });
          router.push("/");
        }}
      >
        Login
      </button>
    </section>
  );
};

export default Unauthorized;
