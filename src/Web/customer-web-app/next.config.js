/** @type {import('next').NextConfig} */

const nextConfig = {
  reactStrictMode: true,
  swcMinify: true,
  output: "standalone",
  assetPrefix: "/site-customers",
};

module.exports = nextConfig;
