import { NextApiRequest, NextApiResponse } from "next";
import { authAxios } from "../axios";
import cookie from "cookie";

export default async function handler(
  req: NextApiRequest,
  res: NextApiResponse
) {
  try {
    const { data, headers: returnedHeaders } = await authAxios.post(
      "/sso",
      JSON.stringify({
        userId: 12345,
        username: "alvin",
        roleId: 4,
        subAccId: 0,
      }),
      {
        headers: { "Content-Type": "application/json" },
      }
    );
    console.log(data);
    Object.entries(returnedHeaders).forEach((keyArr) =>
      res.setHeader(keyArr[0], keyArr[1] as string)
    );

    res.setHeader(
      "Set-Cookie",
      cookie.serialize("auth", data, {
        httpOnly: true,
        secure: process.env.NODE_ENV !== "development",
        sameSite: "strict",
        maxAge: 3600,
        path: "/",
      })
    );

    return res.send(data);
  } catch ({ response: { status, data } }) {
    // Send status (probably 401) so the axios interceptor can run.
    res.status(401).json(data);
  }
}
