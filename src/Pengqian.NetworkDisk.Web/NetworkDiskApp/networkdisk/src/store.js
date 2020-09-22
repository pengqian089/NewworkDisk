import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex);

const store = new Vuex.Store({
    state : {
        title: "网络磁盘",
        userInfo : {
            expire : '',
            token : '',
            account : '',
        }
    },
    getters :{        
        getToken(state){
            return state.userInfo.token;
        },
        getAccount(){
          return localStorage.account;  
        },
        isLogin() {
            let tokenExpire = Date.parse(new Date(store.state.userInfo.expire  || localStorage.tokenExpire));
            let nowTime = Date.parse(new Date());
            console.log('nowTime : ',nowTime,'expire : ',tokenExpire);
            return typeof (localStorage.token) !== "undefined" && localStorage.token !== '' && tokenExpire > nowTime;
        }
    },
    mutations : {
        // 保存jwt认证后的token和expire
        setToken(state,payload) {
            state.userInfo.expire = payload.expire;
            state.userInfo.token = payload.token;
            state.userInfo.account = payload.account;
        }        
    }

});
export default store;