import Vue from 'vue'
import VueRouter from 'vue-router'
import App from './App.vue'
import login from './components/login'

Vue.config.productionTip = false;

const routes = [
    {path: "/login", component: login}
    ];
const router = new VueRouter({routes});
new Vue({
    render: h => h(App),
    router:router
}).$mount('#app')
