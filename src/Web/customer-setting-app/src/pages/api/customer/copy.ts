import type { NextApiRequest, NextApiResponse } from "next";
import { z } from "zod";

const querySchema = z.object({
  username: z.string(),
});

export default function handler(req: NextApiRequest, res: NextApiResponse) {
  res.status(200).json({ name: "API" });
}
