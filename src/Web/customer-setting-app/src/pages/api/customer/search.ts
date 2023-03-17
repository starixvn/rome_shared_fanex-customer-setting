import type { NextApiRequest, NextApiResponse } from "next";

export default async function handler(
  req: NextApiRequest,
  res: NextApiResponse
) {
  const query = req.query["query"];
  const url = `${
    process.env.CUSTOMER_API
  }/search?custId=${14373905}&query=${query}`;
  const result = await fetch(url);
  const customers = await result.json();
  res.status(200).json(customers);
}
