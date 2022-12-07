import axios from "axios";
import { NextApiRequest, NextApiResponse } from "next";
import cookie from "cookie";

export default async function login(req: NextApiRequest, res: NextApiResponse) {
  const { body, headers } = req;
  const { data, headers: returnedHeaders } = await axios.post(
    "http://localhost:5139/api/Auth/sso",
    body,
    { headers }
  );

  Object.entries(returnedHeaders).forEach((keyArr) =>
    res.setHeader(keyArr[0], keyArr[1] as string)
  );

  res.setHeader(
    "Set-Cookie",
    cookie.serialize("accessToken", data.accessToken, {
      httpOnly: true,
      secure: process.env.NODE_ENV !== "development",
      sameSite: "strict",
      maxAge: 3600,
      path: "/",
    })
  );

  res.send(data);
}
