<template>
  <div class="container">
    <div class="code-box">
      <div class="box-header">
        <div class="details-container">
          <div class="desc">
            <a href="javascript:;" style="margin-right: 10px" v-on:click="goBack">返回</a>
            <input type="text" id="txtSearch" v-model="searchText"/> <a href="javascript:;" v-on:click="search">搜索</a>
          </div>
        </div>
      </div>

      <div class="box-rows">
        <div class="row" v-for="file in files" v-bind:todo="file" v-bind:key="file.id">
          <div class="icon">
            <svg height="16" class="git-file" color="gray-light" aria-label="File" viewBox="0 0 16 16" width="16"
                 role="img">
              <path
                  d="M3.75 1.5a.25.25 0 00-.25.25v11.5c0 .138.112.25.25.25h8.5a.25.25 0 00.25-.25V6H9.75A1.75 1.75 0 018 4.25V1.5H3.75zm5.75.56v2.19c0 .138.112.25.25.25h2.19L9.5 2.06zM2 1.75C2 .784 2.784 0 3.75 0h5.086c.464 0 .909.184 1.237.513l3.414 3.414c.329.328.513.773.513 1.237v8.086A1.75 1.75 0 0112.25 15h-8.5A1.75 1.75 0 012 13.25V1.75z"></path>
            </svg>
          </div>
          <div class="header">
            <span>
                <a href="javascript:;" :title="file.fileName">{{ file.fileName }}</a>
            </span>
          </div>
          <div class="desc">
            <span>
                <a :title="file.path.join('/')">{{ file.path.join('/') }}</a>
            </span>
          </div>
          <div class="time">
            <span>
                <a href="javascript:;" v-on:click="delFile(file.path,file.fileName)" style="margin-right: 8px">删除</a>
                <a href="javascript:;" v-on:click="rename(file.path,file.fileName)">重命名</a>
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "Search",
  data() {
    return {
      searchText: "",
      files: []
    }
  },
  mounted: function () {
    this.$nextTick(function () {
      this.searchText = this.$route.query.key;
      this.reload();
    })
  },
  methods: {
    reload: function () {
      let that = this;
      this.request.fetch("/api/disk/search", {key: this.searchText}).then(x => {
        that.files = x;
      });
    },
    goBack() {
      window.history.length > 1 ? this.$router.go(-1) : this.$router.push('/')
    },
    search: function () {
      this.$router.push({path: "search", query: {key: this.searchText}});
      this.reload();
    },
    delFile: function (path, name) {
      let that = this;
      this.request.post("/api/disk/file", {path: path.join("/"), name: name}).then(() => {
        that.reload();
      });
    },
    rename: function (path, fileName) {
      let that = this;
      this.layer.prompt({
        title: "请输入新名称", formType: 1
      }, function (pass, index) {
        let loadIndex = that.layer.loading();
        that.request.post("/api/disk/renameFile",
            {
              path: path.join("/"),
              fileName: fileName,
              newFileName: pass
            })
            .then(() => {
              that.reload();
              that.layer.close(index);
              that.layer.close(loadIndex);
            });
      });
    }
  }
}
</script>

<style scoped>
.container {
  width: 1170px;
  margin: 0 auto;
}

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