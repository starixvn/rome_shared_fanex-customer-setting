/** @type {import('next').NextConfig} */
const crypto = require("crypto");

const nextConfig = {
  reactStrictMode: true,
  swcMinify: true,
  output: "standalone",
  assetPrefix: "/site-customers",
  generateBuildId: async () => {
    return crypto.randomBytes(20).toString("hex");
  },
};

module.exports = nextConfig;
