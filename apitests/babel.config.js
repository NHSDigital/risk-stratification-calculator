module.exports = {
  presets: ['@babel/preset-env'],
  plugins: [
    '@babel/plugin-proposal-throw-expressions'
  ],
  env: {
    test: {
      plugins: ['@babel/plugin-transform-runtime']
    }
  }
};
