import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  output: "export",
  assetPrefix: "./", // Use relative path
  trailingSlash: true,
  images: {
    unoptimized: true,
  },
  // webpack: (config, { isServer }) => {
  //   if (!isServer) {
  //     config.output = config.output || {};
  //     config.output.publicPath = "./_next/"; 
  //   }
  //   return config;
  // },
};

export default nextConfig;
