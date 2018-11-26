import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex);

export default new Vuex.Store({
  strict: process.env.NODE_ENV !== 'production',
  state: {
    instanceName: '',
  },
  mutations: {
    setInstanceName(state, instanceName) {
      state.instanceName = instanceName;
    },
  },
  actions: {
    setInstanceName(context, instanceName) {
      context.commit('setInstanceName', instanceName);
    },
  },
});
