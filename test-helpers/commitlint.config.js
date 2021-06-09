module.exports = {
  plugins: ['commitlint-plugin-jira-rules'],
  extends: ['jira'],
  rules: {
    'jira-task-id-max-length': [2, 'always', 15],
    'jira-task-id-project-key': [2, 'always', ['RISKSTRAT']]
  },
};
