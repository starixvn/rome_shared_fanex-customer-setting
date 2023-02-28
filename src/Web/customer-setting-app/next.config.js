/** @type {import('next').NextConfig} */

const isProd = process.env.NODE_ENV === "production";

const nextConfig = {
  reactStrictMode: true,
  output: "standalone",
  assetPrefix: isProd ? "/customer" : "",
};

module.exports = nextConfig;