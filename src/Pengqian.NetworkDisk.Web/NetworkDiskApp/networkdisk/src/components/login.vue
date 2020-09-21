<template>
  <div class="beg-login-bg">
    <div class="beg-login-box">
      <div class="beg-login-main">        
          <div style="text-align: right;margin-top: 10px">
            <label class="beg-login-icon" for="account" style="float: left">账号</label>
            <input type="text" v-model="account" id="account" required="required" autocomplete="off" placeholder="账号" >
          </div>
          <div style="text-align: right;margin-top: 10px">
            <label for="password" class="beg-login-icon" style="float: left">密码</label>
            <input type="password" v-model="password" id="password" required="required" autocomplete="off" placeholder="密码">
          </div>
          <div>
            <button class="login-btn" type="button" v-on:click="login">登&nbsp;&nbsp;录</button>
          </div>   
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "login",
  data() {
    return {
      account: "",
      password: ""
    }
  },
  methods: {
    login: function () {
      if(this.account === ""){
        this.layer.msg("请输入账号！");
        return;
      }
      if(this.password === ""){
        this.layer.msg("请输入密码！");
        return;
      }
      let that = this;
      this.request.post("/api/Account",
          {
            account: this.account,
            password: this.password
          })
          .then(x => {
            this.$store.commit('setToken', x);
            localStorage.account = x.account;
            localStorage.tokenExpire = x.expires;
            localStorage.token = x.token;
            this.$router.push({path: '/'});
          }).catch(x => {
        if (x.response.status === 400) {
          that.layer.msg("用户名或密码错误！");
        } else {
          that.layer.alert(x);
        }
      });
    }
  }
}
</script>

<style scoped>


.beg-login-box {
  width: 360px;
  margin: 5% 0 0 5%;
  background-color: rgba(0, 0, 0, 0.407843);
  border-radius: 10px;
  color: aliceblue;
}

.beg-login-box header {
  height: 39px;
  padding: 10px;
  border-bottom: 1px solid aliceblue;
}

.beg-login-box header h1 {
  text-align: center;
  font-size: 25px;
  line-height: 40px;
}

.beg-login-box .beg-login-main {
  padding: 50px 40px;
}

.beg-login-main .layui-form-item input {
  padding-left: 34px;
}

.beg-login-box footer {
  height: 35px;
  padding: 10px 10px 0 10px;
}

.beg-login-box footer p {
  line-height: 35px;
  text-align: center;
}

.beg-login-code-box input {
  position: absolute;
  width: 100px;
}

.beg-login-code-box img {
  cursor: pointer;
  position: absolute;
  left: 115px;
  height: 38px;
}


.beg-login-box .login-btn {
  margin-top: 10px;
  width: 100%;
  background-color: rgba(34, 33, 105, 0.5);
  color:#fff
}
</style>