### 实现业务的服务层，专注于业务，不再抽象，不使用接口

+ 服务层基础`BasicService`,包含：
    - 数据库配置
    - 数据仓储
    - Mapper，数据实体与ViewModel互相转换
    - 基础函数
 
服务层所有相关业务需要继承`BasicService`,而且实体类型必须实现`IBaseEntity`接口，有默认的构造函数。

``` cs
public class xxService:BasicService<DbEntity>
{

}
```