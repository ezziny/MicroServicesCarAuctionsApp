import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  /* config options here */
  reactCompiler: true,
  logging: {
    fetches:{
    fullUrl: true,
  }
},
images: {
  remotePatterns: [
    {
      protocol: 'https', hostname: 'cdn.pixabay.com'
    },
  ],
}
};

export default nextConfig;
