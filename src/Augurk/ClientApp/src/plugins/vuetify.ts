import Vue from 'vue';
import Vuetify from 'vuetify';
import 'vuetify/src/stylus/app.styl';

import colors from 'vuetify/es5/util/colors';

Vue.use(Vuetify, {
  iconfont: 'md',
  theme: {
    primary: colors.green,
  },
});
