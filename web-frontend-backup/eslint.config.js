import vue from 'eslint-plugin-vue';
import vueTs from '@vue/eslint-config-typescript';
import prettier from '@vue/eslint-config-prettier';

const prettierConfigs = Array.isArray(prettier) ? prettier : [prettier];

export default [
  { files: ['**/*.{js,jsx,ts,tsx,vue}'] },
  ...vue.configs['flat/recommended'],
  ...vueTs(),
  ...prettierConfigs,
];
