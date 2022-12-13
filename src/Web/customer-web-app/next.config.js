/** @type {import('next').NextConfig} */
const execSync = require("child_process").execSync;

const lastCommitCommand = "git rev-parse HEAD";

const nextConfig = {
  reactStrictMode: true,
  swcMinify: true,
  output: "standalone",
  generateBuildId: async () => {
    return execSync(lastCommitCommand).toString().trim();
  },
};

module.exports = nextConfig;
