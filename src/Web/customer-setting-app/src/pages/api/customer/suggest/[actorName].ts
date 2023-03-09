import type { NextApiRequest, NextApiResponse } from "next";

export default async function handler(
  req: NextApiRequest,
  res: NextApiResponse
) {
  const actorName = req.query["actorName"];
  const data = {
    actorName: actorName,
    custId: 1176086,
    roleId: 2,
  };
  const url = `${process.env.CUSTOMER_API}/suggest-username`;
  const result = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });
  const suggestName = await result.json();
  res.status(200).json({ ok: true, suggestName: suggestName });
}
