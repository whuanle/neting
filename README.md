### ���� Neting

�տ�ʼ��ʱ���Ǵ���ʹ��΢��ٷ��� Yarp �⣬ʵ��һ�� API ���أ����淢�ֿӱȽ϶࣬Ū�����Ƚ��鷳���ͷ����ˡ�Ŀǰд���˲鿴 Kubernetes Service ��Ϣ������ Route �� Cluster �Ͱ� Kubernetes Service������˵����������˻������֣�����·�ɺͺ�˷���󶨣������ʵ�ֶ�̬·�ɺ�ֱ��ת���ȹ��ܣ�ֻ��Ҫ���չٷ����ĵ��������м�����ɡ�



ԭ������ʹ�� .NET 6 �� AOT(һ��40MB) �����Ǵ�����л����׳���һЩ��������ͻ������⣬��˷����ˣ���Ȼ���� Runtime + Ӧ�� �ķ�ʽ���𣬽����ܹ� 120 MB ���ҡ�

�����Ŀ��ַ��https://github.com/whuanle/neting 

ǰ����Ŀ��ַ��https://github.com/whuanle/neting-web



�����ַ��http://neting.whuanle.cn:30080/

��Ž����������ģ�

![image-20220109130015021](images/image-20220109130015021.png)



Route������Դ��ڣ�֧�� http/https �� gRPC��Route ����ƽ��뼯Ⱥǰ�ķ�����������ΰ󶨺�˷��񣬿������÷����� URL ���������

Cluster����˷���һ�� Cluster ���԰󶨶�����͵ĺ�˷�����Ҫʵ��һ��������ͬ�ĺ�׺���ʶ��΢���񣬻���ͬһ��ʵ�����ؾ���ȣ�

Service���鿴 Kubernetes Service ��һЩ������Ϣ��



Neting Ŀǰֻʵ���˼򵥵����ã����������˽� Yarp �Լ����� Kubernetes API ��������ؼ�Ⱥ��Դ�ȡ�����Ҳ���Դ����˽� etcd ��ʹ�ã�������һ�� Kubernetes Controller Ӧ�á�



���������Ѿ����ˣ����߿ɸ����������������м�����ɡ�

![image-20220109125624029](images/image-20220109125624029.png)



���������β��� Neting ��Ŀ����Ҫ������ǰ�����ü�Ⱥ��



̸̸�� Yarp �Ŀ�����

���ȣ�Yarp �Ĳֿ��ַ�� https://github.com/microsoft/reverse-proxy

�ֿ���΢��Ĺٷ��˺��£����˽⵽����Ϣ������Yarp �ĳ�����Ҫ�ǽ�� Microsoft �ڲ�������Ҳ����˵��Ϊ�˸� Azure �á������֮�������������˸о��ǲ������ ����Դ������Ȼ�϶��ǿ�Դ�ģ�MIT License ��ԴЭ�飬����������Լ�о�΢��İѿ�����ǿ�������Ŀ����һЩ ��˽�ġ� ȥ���ġ�

���������Ŀ���ǳ�����Ŀ��Yarp ֻ��һ���⣬������һ������Ӧ�ã��ܶ�ط���û��ȷ�����ȶ��ԣ�Ҳû�����ܲ��Ա���ȣ�Ҳû�л��ڴ˵ĳ����Ӧ�ó��֡�API �Ϻ��������棬�����ѶȻ��ǱȽϸ��ӵģ������ĵ��ܶ�ط�Ҳû��˵�����



### ���� etcd

Neting ����Ҫ��ƣ��� etcd ��Ϊ���ݴ洢��ˣ����û����������������������͵Ĺ���ʱ��etcd Watch �Զ�֪ͨ Neing ʵ����ˢ����Щ���õ��ڴ��У����� Yarp ʹ��(��ȻҲ���Բ��ŵ��ڴ棬����Ҫʹ�õ�ʱ���Զ��� etcd ��ȡ)��

Neting ʹ�� etcd ��Ϊ�洢��ˣ�etcd ֧�ּ�Ⱥ����������Ϊ�˷���ʹ�õ��� etcd ʵ������Ϊ etcd ����״̬Ӧ�ã������Ҫ��һ���洢������ etcd Ҳ��Ҫ����һ�� service���Ա��ڼ�Ⱥ�з���ʵ����

```
etcd ��Ⱥ(�����ǵ�ʵ����Ⱥ) ->  Service(����Ϊneting-svc)
     ��
PersistentVolumeClaim
     ��
PersistentVolume
```

���������Ŀ�� yaml ���棬�ҵ� etcd.yaml �� etcds.yaml ��neting-etcd.yaml  �����ļ����鿴��β��� etcd ��Ⱥ��

![image-20220109131035871](images/image-20220109131035871.png)



Ϊ�˷��㣬�����Ĵ洢���Ǳ��ؾ����ܿ�ڵ㹲�����ݡ���Ȼ��Ҳ����ͨ���޸� neting-etcd.yaml �ļ���ʹ���������͵ľ�

```yaml
  hostPath:
      # ������Ŀ¼λ�ã���Ҫ����ǰ����
    path: /data/etcd
      # ���ֶ�Ϊ��ѡ
    type: Directory
```



�� neting-etcd.yaml �ļ��ϴ�����Ⱥ��Ȼ�󴴽� etcd ʵ����

```bash
root@master:~/neting# kubectl apply -f neting-etcd.yaml 
service/neting-etcd created
statefulset.apps/neting-etcd created
persistentvolume/neting-etcd created
```



���� etcd ��Ⱥ��ʱ�򣬻ᴴ�������ص���Դ��

```bash
root@master:~/neting# kubectl get statefulset
NAME          READY   AGE
neting-etcd   1/1     36s

root@master:~/neting# kubectl get pv
NAME          CAPACITY   ACCESS MODES   RECLAIM POLICY   STATUS
neting-etcd   1Gi        RWO            Recycle          Available

root@master:~/neting# kubectl get svc
NAME           TYPE        CLUSTER-IP       EXTERNAL-IP   PORT(S)                               AGE
neting-etcd    ClusterIP   10.96.206.255    <none>        2379/TCP,2380/TCP                     54s
```

> ��ʵ������ PersistentVolumeClaim������ִ�� `kubectl get pvc` �鿴��



### ���� secret

secret ����Ҫ�����Ǹ� Neting �ṩ�û���¼���˺����룬��ǰ���ᵽ�� `admin/admin123`���������д���� secret.yaml��

secret.yaml ��ȫ���������£�

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: neting-basic-auth
type: Opaque
stringData:
# Neting �ĵ�¼�˺�����
  NETING_USER: admin
  NETING_PASSWORD: admin123
  NETING_TOKENKEY: dzbfhisj@3411DDFF5%$%^&&
```



������úܼ򵥣����� NETING_TOKENKEY ��ʾǩ�� token ����Կ��Neting ʹ�� Jwt Token ���û�ƾ֤���ڰ䷢ƾ֤���û�ʱ����Ҫ�����û���Ϣǩ����

�ϴ� secret.yaml  ����Ⱥ�У�Ȼ�󴴽�����Secret �е���Ϣ���ջ����� base64��Kubernetes �� etcd ��(ע������ǰ�����д����� etcd)��secret �е���Ϣ�����ջ��Ի�����������ʽ������ Neting Pod �С�

```bash
root@master:~/neting# kubectl apply -f secret.yaml 
secret/neting-basic-auth created

root@master:~/neting# kubectl get secret
NAME                      TYPE                                  DATA   AGE
neting-basic-auth         Opaque                                3      10s
...
data:
  NETING_PASSWORD: YWRtaW4xMjM=
  NETING_TOKENKEY: ZHpiZmhpc2pAMzQxMURERkY1JSQlXiYm
  NETING_USER: YWRtaW4=
kind: Secret
```



### ���� Neting

Neting ��������ϵ���£�

```
Neting -> ����(Secret)
 ��
Service - etcd
 ��
etcd ʵ��
```



Neting ���� ASP.NET Core API + React/Ant Design ��д�� Web ��Ŀ��Ϊ�˽ṹ�򵥣�Neting �� wwwroot Ŀ¼�й���ǰ�˾�̬�ļ����Ա���ͬһ���˿��·��ʣ����Ҽ��ٿ��򡢰� IP �����顣



Neting �ѱ��ϴ��������ƾ���ֿ��У�`docker pull` ��ַ �� `registry.cn-hangzhou.aliyuncs.com/whuanle/neting:review1.0`



Neting ����Ҫ�� Pod �����ӵ� Kubernetes API Server �ģ������Ҫ���� ServiceAccount ����ֱ��ʹ�� Kubeconfig���ź����ǣ�C# �� KubernetesClient �� ServiceAccount  ֧�ֲ����ã����ֻ��ʹ�� Kubeconfig����Ȼֱ��ʹ�� Kubeconfig ���ܻ����һЩ��ȫ���⣬�������� Demo��Neting ֻ��ʹ�� ��ȡ Servive �� Endpoint ���ֵ���Ϣ������Լ�Ⱥ�����޸ġ�ɾ���Ȳ�������������Ҫ���߰�ȫ����Ĳ������ɳ������н�� Kubenetes - C# �� ServiceAccount  ���⡣

> Kubernetes �� etcd �� C#  SDK ���鲻�ѣ����߸���ԭ���м����ʱ�򣬻����� Go ��ȽϺã�C# �ʺ�дҵ��



����� Kubernetes ���������ļ����Ƶ� `/root/.kube/config` �С�ע�⣬��һ��һ��Ҫ�ڻᱻ���� Pod ���еĽڵ��ϴ�����Ϊ��������ļ����ܿ�ڵ�ʹ�á�

```
cp -f /etc/kubernetes/admin.conf /root/.kube/config
```



Ȼ������ Neting��

```
kubectl apply -f neting.yaml
```



���ţ�Ϊ Neting ���� Service���Ա����������ʡ�

```bash
root@master:~/neting# kubectl apply -f neting-svc.yaml 
service/neting created

root@master:~/neting# kubectl get svc -o wide
neting         NodePort    10.102.240.255   <none>        80:30080/TCP                          11s   app=neting
neting-etcd    ClusterIP   10.96.206.255    <none>        2379/TCP,2380/TCP                     31m   app=neting-etcd
```



������ɺ󣬿���ͨ���ڵ�� IP �� 30080 �˿ڣ����ʵ���

![image-20220109101834395](images/image-20220109101834395.png)

���������һ���˵������Ҫ���¼��

�˺�����ֱ��� `admin`��`admin123`��



![image-20220109101940089](images/image-20220109101940089.png)

��¼��ƾ֤��洢�����������У���Ч��Ϊ 7 �졣

![image-20220109102029148](images/image-20220109102029148.png)



��� Service ���Կ�����Ⱥ�е� Service ����Ϣ��

![image-20220109102143781](images/image-20220109102143781.png)



### ʹ��

�������������� Yarp ������� ���á�

Yarp �ķ���������󶨷�Ϊ�������֣�Route �� Cluster��

Cluster ���Ƿ�����ʵ����������һ��Ӧ�ò����� N ��ʵ����ÿ��ʵ������һ�� IP����ô Cluster ��Ҫ��¼����Щʵ���� IP���Ա��ڷ���ʱ��ͨ�����ؾ����㷨ѡ������һ�����ʡ�YARP �������õĸ���ƽ���㷨����ҲΪ�κ��Զ��帺��ƽ�ⷽ���ṩ�˿���չ�ԡ�����Ͳ�չ��������

���߿��Բο� https://microsoft.github.io/reverse-proxy/articles/load-balancing.html



![image-20220109122234526](images/image-20220109122234526.png)



�ҵ� Kubernetes �У��в��� Ingress ʱ������������Ӧ�ã� web1 �� web2���������ʹ��һ�¡�

![image-20220109122543964](images/image-20220109122543964.png)

![image-20220109122649266](images/image-20220109122649266.png)



���ţ����ߵ�����������������һ��������Ӧ�á�

![image-20220109124328624](images/image-20220109124328624.png)





���Ŵ��� Route��

![image-20220109124339629](images/image-20220109124339629.png)

![image-20220109124529000](images/image-20220109124529000.png)



�ǿ��Ի��� Yarp ��Ŀ���һ�� API ���أ����ߴ��� Kubernetes �� Ingress ��ʵ��������ڣ�API ���� + ���ؾ��⡣

![img](images/ingress_abc.com.png)



˵�����㻹���Ա�д���� Dapr �ķ��������ܣ�ʹ�ñ߳�ģʽΪ��Ⱥ�е�Ӧ���ṩ������ʽ�����������



### ����һ����Ŀ

Neting ���Ǻ����Ŀ��NetingCrdBuilder ����ǰ��Ŀ�޹أ��Ǳ��߱������������� Dapr�������Զ�����Դ�Լ��Լ� Kubernetes Operater �õģ�����д�ˣ��Ͳ�д�ˡ�Yarp.ReverseProxy �� Yarp �����⣬Ϊ�˷�����ԺͲ鿴Դ�룬��ͨ�� Nuget ���ã����ǳ�ȡԴ�룬ͨ������ֱ�����á� 

![image-20220109134120100](images/image-20220109134120100.png)

�����Ŀ�󲿷ֶ�д��ע�ͣ�����Ͳ��ٶ�˵�ˡ�

������ڱ��ز��ԺͿ����������Ȱ�ǰ�����Ŀ��������

���ؿ���������Ҫ�ں����Ŀ�� appsettings.json �� appsettings.Development.json �ļ��޸����á�

![image-20220109134551597](images/image-20220109134551597.png)

���� admin.conf �� Kubernetes API Server ���ӵ���֤�����ļ���ͨ�������ļ��� Kuberntes ���ӵ�ʱ�����ͨ����Ȩ������Դ�������� KubernetesExtensions �У���Ҳ����ͨ�� Kubernetes proxy �ȷ�ʽ���� Kubernetes ���п�����

![image-20220109134733235](images/image-20220109134733235.png)



Ȼ����Ҫ��ǰ�˵� Constants.js �ļ��У������㱾�صĺ�˵�ַ��

```
export const Options = {
    // host: "http://127.0.0.1:80"  // ��������
    host: ""                        // ��������
}
```

���ǰ��˶���ͬһ���˿��£��� `host:""` ���ɡ�



