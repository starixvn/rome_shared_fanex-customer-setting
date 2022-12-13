/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  swcMinify: true,
  output: "standalone",
  assetPrefix: "/site-customers",
  generateBuildId: async () => {
    return new Date().getTime();
  },
};

module.exports = nextConfig;
