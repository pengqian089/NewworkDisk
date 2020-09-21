<template>
  <div id="app">
    <transition name="bounce">
      <router-view></router-view>
    </transition>
  </div>
</template>

<script>


export default {
  name: 'App',
  data(){
    return{
      
    }
  },
  computed:{
    isLogin:function(){
      return !!localStorage.token;
    }
  },
  components: {
   
  }
}
</script>

<style>

</style>
