import type { NextApiRequest, NextApiResponse } from "next";
import CryptoJS from "crypto-js";

type AuthData = {
  token: string;
  refreshToken: string;
};

type TelegramBot = {
  botId: number;
  accessToken: string;
};

export default async function handler(
  req: NextApiRequest,
  res: NextApiResponse
) {
  const botId = req.query["botId"] as string;
  const telegramInitData = req.query["telegramInitData"] as string;
  if (botId && telegramInitData) {
    const initData = decodeURIComponent(telegramInitData);
    const botToken = await getBotToken(Number(botId));
    const authData = await verifyWebAppOpenByTelegram(botToken, initData);

    res.status(200).json(authData);
    return;
  }

  res.status(200).json({});
}

const getBotToken = async (botId: number) => {
  const url = `${process.env.TELEGRAM_API_URL}/Bot`;
  const res = await fetch(url);
  const bots = await res.json();
  const bot: TelegramBot = bots.find((b: TelegramBot) => b.botId == botId);
  return bot.accessToken;
};

const verifyWebAppOpenByTelegram = async (
  botToken: string,
  telegramInitData: string
) => {
  const initData = new URLSearchParams(telegramInitData);
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

  if (initData.get("hash") === checkHash) {
    return { token: "secret", refreshToken: "" };
  }

  return {};
};
