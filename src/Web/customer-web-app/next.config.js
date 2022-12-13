/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  swcMinify: true,
  output: "standalone",
  basePath: "/site-customers",
};

module.exports = nextConfig;
