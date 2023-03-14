import { NextApiRequest, NextApiResponse } from "next";
import CryptoJS from "crypto-js";
import { withIronSessionApiRoute } from "iron-session/next";
import { sessionOptions } from "../../lib/session";

type TelegramBot = {
  botId: number;
  accessToken: string;
};

export type Account = {
  userId: number;
  custId: number;
  subAccId: number;
  customerLevel: number;
  language: string;
  botId: number;
};

export default withIronSessionApiRoute(loginRoute, sessionOptions);

async function loginRoute(req: NextApiRequest, res: NextApiResponse) {
  const { telegramInitData } = req.body;
  console.log(telegramInitData);
  try {
    const initData = new URLSearchParams(telegramInitData);
    var user = JSON.parse(initData.get("user") || "");
    console.log(user);
    var account = await getAccount(user.id);
    console.log(account);
    const botToken = await getBotToken(Number(account.botId));
    console.log(botToken);
    const isVerified = await verifyWebAppOpenByTelegram(botToken, initData);

    if (isVerified) {
      console.log(isVerified);
      var data = {
        isLoggedIn: true,
        custId: account.custId,
        subAccId: account.subAccId,
        customerLevel: account.customerLevel,
        language: account.language,
      };
      req.session.user = data;
      await req.session.save();
    }
    console.log(req.session.user);
    res.json(req.session.user);
  } catch (error) {
    res.status(500).json({ message: (error as Error).message });
  }
}

const getBotToken = async (botId: number) => {
  const url = `${process.env.TELEGRAM_API_URL}/Bot`;
  const res = await fetch(url);
  const bots = await res.json();
  const bot: TelegramBot = bots.find((b: TelegramBot) => b.botId == botId);
  return bot.accessToken;
};

const getAccount = async (chatId: number) => {
  const url = `${process.env.TELEGRAM_API_URL}/Setting/${chatId}`;
  const res = await fetch(url);
  return (await res.json()) as Account;
};

const verifyWebAppOpenByTelegram = async (
  botToken: string,
  initData: URLSearchParams
) => {
  let dataCheckString: string[] = [];
  initData.sort();

  initData.forEach((value, key) => {
    key !== "hash" && dataCheckString.push(`${key}=${value}`);
  });

  const secret = CryptoJS.HmacSHA256(botToken, "WebAppData");
  const checkHash = CryptoJS.HmacSHA256(
    dataCheckString.join("\n"),
    secret
  ).toString(CryptoJS.enc.Hex);
  console.log(checkHash);
  console.log(initData.get("hash"));

  return initData.get("hash") === checkHash;
};
