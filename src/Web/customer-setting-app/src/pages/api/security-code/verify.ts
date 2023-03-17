import { withIronSessionApiRoute } from "iron-session/next";
import { NextApiRequest, NextApiResponse } from "next";
import { sessionOptions } from "src/lib/session";

export default withIronSessionApiRoute(verifyRout, sessionOptions);

async function verifyRout(req: NextApiRequest, res: NextApiResponse) {
  if (req.method !== "POST") {
    res.status(405).send("Method not allowed");
    return;
  }

  //const { user } = req.session;

  //   if (!user || user.isLoggedIn === false) {
  //     res.status(401).end();
  //     return;
  //   }

  const securityCode = req.body.securityCode;
  console.log(`Saving ${securityCode} is saved to the subscribers table`);

  return res.json({
    code: 0,
    message: "",
    isSuccessful: true,
  });
}
