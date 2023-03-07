import type { NextApiRequest, NextApiResponse } from "next";
import { z } from "zod";

const querySchema = z.object({
  username: z.string(),
  query: z.string().optional(),
});

export default function handler(req: NextApiRequest, res: NextApiResponse) {
  const { username, query } = querySchema.parse(req.query);
  res.status(200).json({ ok: true, username: username, query: query });
}
