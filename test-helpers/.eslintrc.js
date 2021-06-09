module.exports = {
  root: true,
  env: {
    node: true,
    jest: true,
  },
  extends: [
    'airbnb-base',
    'eslint:recommended',
    'plugin:security/recommended',
  ],
  plugins: [
    'import',
    'security',
  ],
  globals: {
    Atomics: 'readonly',
    SharedArrayBuffer: 'readonly',
  },
  rules: {
    'import/prefer-default-export': 0,
    'linebreak-style': ['error', 'unix'],
    'comma-dangle': ['error', 'only-multiline'],
    'no-nested-ternary': 0,
    'no-underscore-dangle': 0,
    'class-methods-use-this': 0,
    radix: 0
  },
};
