module.exports = {
  verbose: true,
  moduleDirectories: [
    'node_modules'
  ],
  moduleFileExtensions: [
    'js',
  ],
  setupFilesAfterEnv: [
    'jest-allure/dist/setup',
    '<rootDir>/config/setupAllure.js'
  ],
  testMatch: [
    '<rootDir>/__tests__/qCovid/**/*.js',
  ],
  transformIgnorePatterns: [
    '<rootDir>/node_modules/'
  ],
  transform: {
    '^.+\\.(js|jsx)$': 'babel-jest',
  },
  testTimeout: parseInt(process.env.JEST_TIMEOUT) || 240000,
  reporters: [
    'default',
    'jest-junit',
    'jest-allure',
  ],
};
