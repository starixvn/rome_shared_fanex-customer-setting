/** @type {import('next').NextConfig} */
var path = require("path");
const { i18n } = require("./next-i18next.config.js");

const nextConfig = {
  reactStrictMode: true,
  swcMinify: true,
  output: "standalone",
  // basePath: "/site-customers",
  sassOptions: {
    includePaths: [path.join(__dirname, "styles")],
  },
  i18n,
};

module.exports = nextConfig;
