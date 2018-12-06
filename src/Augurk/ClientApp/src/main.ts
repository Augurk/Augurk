import Vue from 'vue';
import './plugins/vuetify';
import App from './App.vue';
import router from './router';
import store from './store';
import VueMarkdown from 'vue-markdown';

Vue.config.productionTip = false;
Vue.component('vue-markdown', VueMarkdown);

new Vue({
  router,
  store,
  render: (h) => h(App),
}).$mount('#app');
