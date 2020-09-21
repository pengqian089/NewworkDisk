import Vue from 'vue'
import router from "./router";
import App from './App.vue'
import axios from 'axios';
import { post, fetch } from "./httpRequest";
import vueLayer from 'vue-layer'
import 'vue-layer/lib/vue-layer.css';
import store from './store';

Vue.config.productionTip = false;

router.beforeEach((to,from,next) => {
    if(to.meta.auth){
        if (store.getters.isLogin){
            next();
        }else {
            next({
                path : '/login',
                query : {redirect : to.fullPath}
            })
        }
    }else {
        next();
    }
});
new Vue({
    render: h => h(App),
    router:router,
    store
}).$mount('#app');
Vue.prototype.$axios = axios;
Vue.prototype.request = {
    post: post,
    fetch : fetch
};
Vue.prototype.layer = vueLayer(Vue);

