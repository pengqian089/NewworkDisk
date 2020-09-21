<template>
  <h1>network disk</h1>
</template>

<script>
export default {
  data(){
    return{
      directoryInfo:{
        isRoot :false,
        directories:[],
        files:[],
        prevUrl:[],
        currentPath:[]
      }
      
    };
  },
  mounted: function () {
    this.$nextTick(function () {
      console.log("start");
    })
  },
  name: "NetworkDisk"
}
</script>

<style scoped>
.git-dir {
  box-sizing: border-box;
  color: rgb(121, 184, 255);
  display: inline-block;
  fill: rgb(121, 184, 255);
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Helvetica, Arial, sans-serif, "Apple Color Emoji", "Segoe UI Emoji";
  font-size: 14px;
  height: 16px;
  line-height: 21px;
  list-style-type: none;
  overflow-wrap: break-word;
  overflow-x: hidden;
  overflow-y: hidden;
  text-size-adjust: 100%;
  vertical-align: text-bottom;
  width: 16px;
}

.git-file {
  box-sizing: border-box;
  color: rgb(106, 115, 125);
  display: inline-block;
  fill: rgb(106, 115, 125);
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Helvetica, Arial, sans-serif, "Apple Color Emoji", "Segoe UI Emoji";
  font-size: 14px;
  height: 16px;
  line-height: 21px;
  list-style-type: none;
  overflow-wrap: break-word;
  overflow-x: hidden;
  overflow-y: hidden;
  text-size-adjust: 100%;
  vertical-align: text-bottom;
  width: 16px;
}

.code-box {
  margin-top: 10px;
  background-color: #fff;
  border: 1px solid #e1e4e8;
  border-radius: 6px;
  box-sizing: border-box
}

.code-box .box-header {
  background-color: #f1f8ff;
  padding: 16px;
  margin: -1px -1px 0;
  border: 1px solid #c8e1ff;
  border-top-left-radius: 6px;
  border-top-right-radius: 6px;
  display: flex;
}

.code-box .box-header .details-container {
  align-items: center;
  display: flex;
}

.details-container .icon {
  margin: -4px !important;
  display: flex;
}

.details-container .desc {
  margin-left: 16px;
  min-width: 0;
  display: flex;
}

.code-box .box-rows {
  display: block;
  box-sizing: border-box;
  font-size: 14px;
  line-height: 1.5;
  color: #24292e;
}

.code-box .box-rows .row {
  display: flex;
  margin-top: -1px;
  list-style-type: none;
  border-top: 1px solid #e1e4e8;
  box-sizing: border-box;
  height: 38px;
  padding: 8px 16px;
}

.code-box .box-rows .row:hover {
  background-color: #f6f8fa;
}

.code-box .box-rows .row:last-of-type {
  border-bottom-right-radius: 6px;
  border-bottom-left-radius: 6px;
  border-bottom: 1px solid #e1e4e8;
}

.code-box .box-rows .row:last-child {
  border-bottom: 1px solid #e1e4e8;
}

.code-box .box-rows .row .prev {
}

.code-box .box-rows .row .icon {
  width: 16px;
  margin-right: 16px;
  box-sizing: border-box;
}

.code-box .box-rows .row .header {
  margin-right: 16px;
  min-width: 0;
  flex: auto;
}

.code-box .box-rows .row .header span {
  vertical-align: top;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  display: block;
  max-width: 100%;
  box-sizing: border-box;
}

.code-box .box-rows .row .header a {
  color: #24292e;
  text-decoration: none;
  background-color: initial;
  box-sizing: border-box
}

.code-box .box-rows .row .header a:hover {
  color: #0366d6;
  text-decoration: underline;
  outline-width: 0;
}

.code-box .box-rows .row .desc {
  display: block;
  margin-right: 16px;
  min-width: 0;
  flex: auto;
  width: 41.66667%;
  box-sizing: border-box;
}

.code-box .box-rows .row .desc span {
  vertical-align: top;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  display: block;
  max-width: 100%;
  box-sizing: border-box;
}

.code-box .box-rows .row .time {
  width: 120px;
  text-align: right;
  color: #6a737d;
}

.code-box .box-rows pre {
  font-weight: bold
}

</style>