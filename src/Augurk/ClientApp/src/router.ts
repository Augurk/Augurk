import Vue from 'vue';
import Router from 'vue-router';
import Home from './views/Home.vue';

Vue.use(Router);

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home,
    },
    {
      path: '/products/:name',
      name: 'product',
      props: true,
      component: () => import(/* webpackChunkName: "product" */ './views/Product.vue'),
    },
    {
      path: '/products/:productName/groups/:groupName/features/:featureName',
      name: 'feature',
      props: true,
      component: () => import(/* webpackChunkName: "feature" */ './views/Feature.vue'),
    },
  ],
});
