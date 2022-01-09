# neting
1����ɴ��� Cluster��Route���洢�� ETCD������ Watch��ˢ�����ݵ��ڴ�
2����� Cluster��Route ����ӳ�䵽  Yarp
3������ǰ�˻���



��ʼ����������

```
set NETING_USER=admin
set NETING_PASSWORD=admin123
set NETING_TOKENKEY="dzbfhisj@3411DDFF5%$%^&&"
```





Neting ʹ�� etcd ��Ϊ�洢��ˣ�etcd ֧�ּ�Ⱥ����������Ϊ�˷���ʹ�õ��� etcd ʵ������Ϊ etcd ����״̬Ӧ�ã������Ҫ��һ���洢������ etcd Ҳ��Ҫ����һ�� service���Ա��ڼ�Ⱥ�з���ʵ����

```
etcd ��Ⱥ(�����ǵ�ʵ����Ⱥ) ->  Service(����Ϊneting-svc)
     ��
PersistentVolumeClaim
     ��
PersistentVolume
```



Ϊ�˷��㣬�����Ĵ洢���Ǳ��ؾ����ܿ�ڵ㹲�����ݡ���Ȼ��Ҳ����ͨ���޸� neting-etcd.yaml �ļ���ʹ���������͵ľ�

```
  hostPath:
      # ������Ŀ¼λ�ã���Ҫ����ǰ����
    path: /data/etcd
      # ���ֶ�Ϊ��ѡ
    type: Directory
```







������� neting-etcd.yaml �п��������õ�ȫ����

```bash
root@master:~/neting# kubectl apply -f neting-etcd.yaml 
service/neting-etcd created
statefulset.apps/neting-etcd created
persistentvolume/neting-etcd created
```



Ҳ˳��������������Դ��

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



Neting ��Ҫ֪�� etcd �� IP ��ַ���Լ����� Neting �������ã����¼�˺�����ȡ�

�������д����secret.yaml��



```
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

Secret �е���Ϣ���ջ����� base64���洢�� etcd �С�

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





```
Neting -> ����(Secret)
 ��
Service - etcd
 ��
etcd ʵ��
```



Neting ���� ASP.NET Core API + React/Ant Design ��д�� Web ��Ŀ��Ϊ�˽ṹ�򵥣�Neting �� wwwroot Ŀ¼�й���ǰ�˾�̬�ļ����Ա���ͬһ���˿��·��ʣ����Ҽ��ٿ��򡢰� IP �����顣



Neting �ѱ��ϴ��� registry.cn-hangzhou.aliyuncs.com/whuanle/neting:review1.0

Neting ����Ҫ�� Pod �����ӵ� Kubernetes API Server �ģ������Ҫ���� ServiceAccount ����ֱ��ʹ�� Kubeconfig���ź����ǣ�C# �� KubernetesClient �� ServiceAccount  ֧�ֲ����ã����ֻ��ʹ�� Kubeconfig����Ȼֱ��ʹ�� Kubeconfig ���ܻ����һЩ��ȫ���⣬�������� Demo��Neting ֻ��ʹ�� ��ȡ Servive �� Endpoint ���ֵ���Ϣ������Լ�Ⱥ�����޸ġ�ɾ���Ȳ�������������Ҫ���߰�ȫ����Ĳ������ɳ������н�� Kubenetes - C# �� ServiceAccount  ���⡣��



����� Kubernetes ���������ļ����Ƶ� `/root/.kube/config` �С�ע�⣬��һ��һ��Ҫ�ڻᱻ���� Pod ���еĽڵ��ϴ�����Ϊ������ܿ�ڵ�ʹ��

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



![image-20220109101834395](images/image-20220109101834395.png)

���������һ���˵������Ҫ���¼��

�˺�����ֱ��� `admin`��`admin123`��



![image-20220109101940089](images/image-20220109101940089.png)

��¼��ƾ֤��洢�����������У���Ч��Ϊ 7 �졣

![image-20220109102029148](images/image-20220109102029148.png)



��� Service ���Կ�����Ⱥ�е� Service��

![image-20220109102143781](images/image-20220109102143781.png)



���������������������

 

Yarp �ķ���������󶨷�Ϊ�������֣�Route �� Cluster��

Cluster ���Ƿ�����ʵ����������һ��Ӧ�ò����� N ��ʵ����ÿ��ʵ������һ�� IP����ô Cluster ��Ҫ��¼����Щʵ���� IP���Ա��ڷ���ʱ��ͨ�����ؾ����㷨ѡ������һ�����ʡ�YARP �������õĸ���ƽ���㷨����ҲΪ�κ��Զ��帺��ƽ�ⷽ���ṩ�˿���չ�ԡ�����Ͳ�չ��������

���߿��Բο� https://microsoft.github.io/reverse-proxy/articles/load-balancing.html



![image-20220109122234526](images/image-20220109122234526.png)





![image-20220109122543964](images/image-20220109122543964.png)

![image-20220109122649266](images/image-20220109122649266.png)



![image-20220109124328624](images/image-20220109124328624.png)



![image-20220109124339629](images/image-20220109124339629.png)

![image-20220109124529000](images/image-20220109124529000.png)
