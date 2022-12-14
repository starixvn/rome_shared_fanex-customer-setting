module.exports = {
  plugins: {
    "postcss-url": {
      url: (asset) => {
        console.log(asset.url);
        if (asset.url[0] === "/") {
          return "https://mb4.nexdev.net/site-customers" + asset.url;
        }
        return asset.url;
      },
    },
  },
};
